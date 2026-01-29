"""Lightweight checks for the consolidated A2A server entrypoint."""

import pathlib
import sys

import main

sys.path.append(str(pathlib.Path(__file__).resolve().parents[1]))


def test_exports_available() -> None:
    assert callable(main.create_server_app)
    assert callable(main.run_server)
