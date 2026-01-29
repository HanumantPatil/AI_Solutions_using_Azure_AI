"""Placeholder to document FastAPI removal.

The FastAPI-based implementation has been removed in favor of the official A2A
SDK server. Tests that relied on the FastAPI routes now live in
`tests/test_server.py`.
"""

import pytest


@pytest.mark.skip(reason="FastAPI implementation removed; see tests/test_server.py")
def test_fastapi_removed() -> None:
    assert True
