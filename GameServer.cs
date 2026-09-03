using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

namespace WinForm
{
    /// <summary>
    /// server.js を C# で書き換えたもの。
    /// Node.js が無い PC でも「メモリーゲーム」を配信できるようにする軽量 HTTP サーバー。
    /// - GET /                … index.html
    /// - GET /index.html      … index.html
    /// - GET /api/images      … Images フォルダ内の画像一覧 (JSON)
    /// - GET /その他          … ルート配下の静的ファイル
    /// </summary>
    internal sealed class GameServer
    {
        private static readonly Dictionary<string, string> Mime =
            new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { ".html", "text/html; charset=utf-8" },
            { ".js",   "text/javascript; charset=utf-8" },
            { ".css",  "text/css; charset=utf-8" },
            { ".jpg",  "image/jpeg" },
            { ".jpeg", "image/jpeg" },
            { ".png",  "image/png" },
            { ".gif",  "image/gif" },
            { ".webp", "image/webp" },
            { ".svg",  "image/svg+xml" },
            { ".bmp",  "image/bmp" },
            { ".ico",  "image/x-icon" },
        };

        private static readonly Regex ImagePattern =
            new Regex(@"\.(jpe?g|png|gif|webp|svg|bmp)$", RegexOptions.IgnoreCase);

        private HttpListener _listener;
        private Thread _thread;
        private volatile bool _running;

        /// <summary>実際に待ち受けているポート番号。</summary>
        public int Port { get; private set; }

        /// <summary>index.html / Images フォルダを含む配信ルート。</summary>
        public string Root { get; private set; }

        public bool IsRunning { get { return _running; } }

        public string Url { get { return "http://localhost:" + Port + "/"; } }

        /// <summary>
        /// サーバーを起動する。<paramref name="preferredPort"/> から順にポートを試し、
        /// 使用中なら次のポートへフォールバックする。
        /// </summary>
        public void Start(int preferredPort, int attempts = 50)
        {
            if (_running) return;

            Root = ResolveRoot();

            Exception last = null;
            for (int i = 0; i < attempts; i++)
            {
                int port = preferredPort + i;
                var listener = new HttpListener();
                listener.Prefixes.Add("http://localhost:" + port + "/");
                try
                {
                    listener.Start();
                    _listener = listener;
                    Port = port;
                    break;
                }
                catch (HttpListenerException ex)
                {
                    // ポート使用中など。次のポートを試す。
                    last = ex;
                    listener.Close();
                }
                catch (SocketException ex)
                {
                    last = ex;
                    listener.Close();
                }
            }

            if (_listener == null)
            {
                throw new InvalidOperationException(
                    "利用可能なポートが見つかりませんでした。", last);
            }

            _running = true;
            _thread = new Thread(Loop) { IsBackground = true, Name = "GameServer" };
            _thread.Start();
        }

        /// <summary>サーバーを停止する。起動していなければ何もしない。</summary>
        public void Stop()
        {
            if (!_running && _listener == null) return;

            _running = false;

            if (_listener != null)
            {
                try { _listener.Stop(); } catch { }
                try { _listener.Close(); } catch { }
                _listener = null;
            }

            if (_thread != null)
            {
                try { _thread.Join(2000); } catch { }
                _thread = null;
            }
        }

        private void Loop()
        {
            while (_running)
            {
                HttpListenerContext ctx;
                try
                {
                    ctx = _listener.GetContext();
                }
                catch
                {
                    // Stop() による Close で GetContext が例外を投げる → 終了。
                    break;
                }

                ThreadPool.QueueUserWorkItem(state =>
                {
                    var c = (HttpListenerContext)state;
                    try { Handle(c); }
                    catch
                    {
                        try { c.Response.Abort(); } catch { }
                    }
                }, ctx);
            }
        }

        private void Handle(HttpListenerContext ctx)
        {
            HttpListenerRequest req = ctx.Request;
            HttpListenerResponse res = ctx.Response;

            string urlPath = Uri.UnescapeDataString(req.Url.AbsolutePath);

            // 画像一覧 API
            if (urlPath == "/api/images")
            {
                res.AddHeader("Access-Control-Allow-Origin", "*");
                res.ContentType = "application/json; charset=utf-8";
                WriteBytes(res, Encoding.UTF8.GetBytes(BuildImageJson()));
                return;
            }

            // 配信するファイルパスを決定
            string filePath;
            if (urlPath == "/" || urlPath == "/index.html")
            {
                filePath = Path.Combine(Root, "index.html");
            }
            else
            {
                string rel = urlPath.TrimStart('/', '\\').Replace('/', Path.DirectorySeparatorChar);
                filePath = Path.GetFullPath(Path.Combine(Root, rel));

                // ルート外へのアクセス (ディレクトリトラバーサル) を拒否
                string rootWithSep = EnsureTrailingSeparator(Path.GetFullPath(Root));
                if (!filePath.StartsWith(rootWithSep, StringComparison.OrdinalIgnoreCase))
                {
                    SendNotFound(res);
                    return;
                }
            }

            if (!File.Exists(filePath))
            {
                SendNotFound(res);
                return;
            }

            string ext = Path.GetExtension(filePath);
            string mime;
            res.ContentType = Mime.TryGetValue(ext, out mime) ? mime : "application/octet-stream";
            WriteBytes(res, File.ReadAllBytes(filePath));
        }

        private string BuildImageJson()
        {
            string imgDir = Path.Combine(Root, "Images");
            var names = new List<string>();

            if (Directory.Exists(imgDir))
            {
                foreach (string f in Directory.GetFiles(imgDir))
                {
                    string name = Path.GetFileName(f);
                    if (ImagePattern.IsMatch(name)) names.Add(name);
                }
                names.Sort(StringComparer.Ordinal);
            }

            var sb = new StringBuilder();
            sb.Append("{\"Files\":[");
            for (int i = 0; i < names.Count; i++)
            {
                if (i > 0) sb.Append(',');
                sb.Append("{\"Name\":\"").Append(JsonEscape(names[i])).Append("\"}");
            }
            sb.Append("]}");
            return sb.ToString();
        }

        /// <summary>
        /// index.html を含むフォルダを探す。exe と同じ場所から親方向へ辿る。
        /// (開発時は bin\Debug から プロジェクトルートの index.html を見つける)
        /// </summary>
        private static string ResolveRoot()
        {
            var startPoints = new List<string>();

            string asmDir = Path.GetDirectoryName(typeof(GameServer).Assembly.Location);
            if (!string.IsNullOrEmpty(asmDir)) startPoints.Add(asmDir);

            string baseDir = AppDomain.CurrentDomain.BaseDirectory;
            if (!string.IsNullOrEmpty(baseDir)) startPoints.Add(baseDir);

            startPoints.Add(Directory.GetCurrentDirectory());

            foreach (string start in startPoints)
            {
                for (DirectoryInfo dir = new DirectoryInfo(start); dir != null; dir = dir.Parent)
                {
                    if (File.Exists(Path.Combine(dir.FullName, "index.html")))
                        return dir.FullName;
                }
            }

            return startPoints[0];
        }

        private static string EnsureTrailingSeparator(string path)
        {
            return path.EndsWith(Path.DirectorySeparatorChar.ToString())
                ? path
                : path + Path.DirectorySeparatorChar;
        }

        private static void SendNotFound(HttpListenerResponse res)
        {
            res.StatusCode = 404;
            res.ContentType = "text/plain; charset=utf-8";
            WriteBytes(res, Encoding.UTF8.GetBytes("Not found"));
        }

        private static void WriteBytes(HttpListenerResponse res, byte[] data)
        {
            res.ContentLength64 = data.Length;
            using (Stream os = res.OutputStream)
            {
                os.Write(data, 0, data.Length);
            }
        }

        private static string JsonEscape(string s)
        {
            var sb = new StringBuilder(s.Length + 8);
            foreach (char c in s)
            {
                switch (c)
                {
                    case '"':  sb.Append("\\\""); break;
                    case '\\': sb.Append("\\\\"); break;
                    case '\b': sb.Append("\\b"); break;
                    case '\f': sb.Append("\\f"); break;
                    case '\n': sb.Append("\\n"); break;
                    case '\r': sb.Append("\\r"); break;
                    case '\t': sb.Append("\\t"); break;
                    default:
                        if (c < ' ') sb.Append("\\u").Append(((int)c).ToString("x4"));
                        else sb.Append(c);
                        break;
                }
            }
            return sb.ToString();
        }
    }
}
