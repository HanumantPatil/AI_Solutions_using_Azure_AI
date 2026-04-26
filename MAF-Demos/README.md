# MAF-Demos

Workflow and orchestration demos using the Microsoft Agent Framework Python SDK.

## Main Example

* `executors_and_edges.py`: Demonstrates executor nodes, handler typing, workflow edges, and output visualization.

## Prerequisites

* Python 3.10+
* Graphviz installed (required for SVG export)

## Setup

```bash
cd MAF-Demos
pip install -r requirements.txt
```

## Run

```bash
cd MAF-Demos
python executors_and_edges.py
```

## Output

* Prints Mermaid workflow definitions to console
* Saves SVG diagrams to `docs/` when Graphviz is available
