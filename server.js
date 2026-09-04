const http = require("http");
const fs = require("fs");
const path = require("path");

const ROOT = __dirname;
const PORT = 8080;
const IMG_DIR = path.join(ROOT, "Images");

const MIME = {
  ".html": "text/html; charset=utf-8",
  ".js": "text/javascript; charset=utf-8",
  ".css": "text/css; charset=utf-8",
  ".jpg": "image/jpeg",
  ".jpeg": "image/jpeg",
  ".png": "image/png",
  ".gif": "image/gif",
  ".webp": "image/webp",
  ".svg": "image/svg+xml",
  ".bmp": "image/bmp",
  ".ico": "image/x-icon"
};

function listImages(cb) {
  fs.readdir(IMG_DIR, (err, files) => {
    if (err) return cb(null, []);
    const imgs = files.filter(f => /\.(jpe?g|png|gif|webp|svg|bmp)$/i.test(f)).sort();
    cb(null, imgs);
  });
}

const server = http.createServer((req, res) => {
  const urlPath = decodeURIComponent(req.url.split("?")[0]);

  if (urlPath === "/api/images") {
    listImages((err, imgs) => {
      res.setHeader("Content-Type", "application/json; charset=utf-8");
      res.setHeader("Access-Control-Allow-Origin", "*");
      res.end(JSON.stringify({ Files: imgs.map(name => ({ Name: name })) }));
    });
    return;
  }

  let filePath;
  if (urlPath === "/" || urlPath === "/index.html") {
    filePath = path.join(ROOT, "index.html");
  } else {
    const rel = path.normalize(urlPath).replace(/^(\.[\/\\])+/, "");
    filePath = path.join(ROOT, rel);
  }

  fs.readFile(filePath, (err, data) => {
    if (err) {
      res.statusCode = 404;
      res.setHeader("Content-Type", "text/plain; charset=utf-8");
      res.end("Not found");
      return;
    }
    const ext = path.extname(filePath).toLowerCase();
    res.setHeader("Content-Type", MIME[ext] || "application/octet-stream");
    res.end(data);
  });
});

server.listen(PORT, () => {
  console.log("Memory game server: http://localhost:" + PORT);
  console.log("Images folder: " + IMG_DIR);
});
