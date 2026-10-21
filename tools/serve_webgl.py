"""Loopback-only Unity WebGL validation host; never publishes the build."""
import argparse
import functools
from http.server import SimpleHTTPRequestHandler, ThreadingHTTPServer
from pathlib import Path
from urllib.parse import urlsplit


class UnityHandler(SimpleHTTPRequestHandler):
    def guess_type(self, path):
        if path.endswith((".br", ".gz")):
            path = path.rsplit(".", 1)[0]
        if path.endswith(".wasm"):
            return "application/wasm"
        if path.endswith(".data"):
            return "application/octet-stream"
        return super().guess_type(path)

    def end_headers(self):
        path = urlsplit(self.path).path
        if path.endswith(".br"):
            self.send_header("Content-Encoding", "br")
        elif path.endswith(".gz"):
            self.send_header("Content-Encoding", "gzip")
        self.send_header("Cross-Origin-Opener-Policy", "same-origin")
        self.send_header("Cross-Origin-Embedder-Policy", "require-corp")
        self.send_header("Cache-Control", "no-store")
        super().end_headers()


if __name__ == "__main__":
    parser = argparse.ArgumentParser()
    parser.add_argument("directory", nargs="?", default="Builds/WebGL")
    parser.add_argument("--port", type=int, default=8765)
    args = parser.parse_args()
    root = Path(args.directory).resolve()
    if not (root / "index.html").is_file():
        parser.error("Build directory must contain index.html")
    handler = functools.partial(UnityHandler, directory=str(root))
    print(f"WebGL validation host: http://127.0.0.1:{args.port}", flush=True)
    with ThreadingHTTPServer(("127.0.0.1", args.port), handler) as server:
        server.serve_forever()
