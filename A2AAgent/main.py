"""Deprecated FastAPI entrypoint.

The FastAPI implementation has been removed. Use the A2A SDK server defined in
`server.py` instead. This file remains only as a convenience shim so existing
invocations continue to start the official SDK-based server.
"""

from server import run_server


def main() -> None:
    """Start the A2A SDK server."""

    run_server()


if __name__ == "__main__":
    main()
