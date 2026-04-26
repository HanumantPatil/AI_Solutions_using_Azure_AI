# LangChainApp

LangChain demo application for Azure OpenAI models deployed in Azure AI Foundry.

## What It Demonstrates

* Simple chat invocation
* Prompt templates with LCEL chain composition
* Multi-turn conversation history

## Files

* `app.py`: Main demo script
* `requirements.txt`: Python dependencies
* `.env.example`: Environment variable template

## Prerequisites

* Python 3.10+
* Azure OpenAI endpoint and deployment

## Setup

```bash
cd LangChainApp
pip install -r requirements.txt
```

Create a `.env` file (or set environment variables) with:

* `AZURE_OPENAI_ENDPOINT`
* `AZURE_OPENAI_API_KEY`
* `AZURE_OPENAI_DEPLOYMENT_NAME` (optional, defaults to `gpt-4o`)
* `AZURE_OPENAI_API_VERSION` (optional)

## Run

```bash
cd LangChainApp
python app.py
```
