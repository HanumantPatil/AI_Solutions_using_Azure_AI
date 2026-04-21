"""
LangChain demo — GPT model deployed in Azure AI Foundry
--------------------------------------------------------
Demonstrates:
  1. Simple chat invocation
  2. Prompt template + chain
  3. Multi-turn conversation with message history
"""

import os
from dotenv import load_dotenv

from langchain_openai import AzureChatOpenAI
from langchain_core.messages import HumanMessage, SystemMessage, AIMessage
from langchain_core.prompts import ChatPromptTemplate
from langchain_core.output_parsers import StrOutputParser

# ---------------------------------------------------------------------------
# Configuration
# ---------------------------------------------------------------------------
load_dotenv()

AZURE_OPENAI_ENDPOINT = os.environ["AZURE_OPENAI_ENDPOINT"]
AZURE_OPENAI_API_KEY = os.environ["AZURE_OPENAI_API_KEY"]
AZURE_OPENAI_API_VERSION = os.getenv("AZURE_OPENAI_API_VERSION", "2024-08-01-preview")
AZURE_OPENAI_DEPLOYMENT_NAME = os.getenv("AZURE_OPENAI_DEPLOYMENT_NAME", "gpt-4o")


def get_llm() -> AzureChatOpenAI:
    """Create and return an AzureChatOpenAI client pointed at the Foundry deployment."""
    return AzureChatOpenAI(
        azure_endpoint=AZURE_OPENAI_ENDPOINT,
        api_key=AZURE_OPENAI_API_KEY,
        api_version=AZURE_OPENAI_API_VERSION,
        azure_deployment=AZURE_OPENAI_DEPLOYMENT_NAME,
        temperature=0.7,
        max_tokens=1024,
    )


# ---------------------------------------------------------------------------
# Demo 1: Simple chat invocation
# ---------------------------------------------------------------------------
def demo_simple_chat(llm: AzureChatOpenAI) -> None:
    print("\n" + "=" * 60)
    print("Demo 1: Simple Chat Invocation")
    print("=" * 60)

    messages = [
        SystemMessage(content="You are a helpful assistant that explains concepts clearly and concisely."),
        HumanMessage(content="What is Azure AI Foundry and how does it help developers?"),
    ]

    response = llm.invoke(messages)
    print(f"Response:\n{response.content}")


# ---------------------------------------------------------------------------
# Demo 2: Prompt template + LCEL chain
# ---------------------------------------------------------------------------
def demo_prompt_chain(llm: AzureChatOpenAI) -> None:
    print("\n" + "=" * 60)
    print("Demo 2: Prompt Template + Chain (LCEL)")
    print("=" * 60)

    prompt = ChatPromptTemplate.from_messages([
        ("system", "You are an expert in {domain}. Provide a concise answer in {language}."),
        ("human", "{question}"),
    ])

    chain = prompt | llm | StrOutputParser()

    result = chain.invoke({
        "domain": "cloud computing",
        "language": "English",
        "question": "What are the key benefits of deploying AI models in the cloud?",
    })

    print(f"Response:\n{result}")


# ---------------------------------------------------------------------------
# Demo 3: Multi-turn conversation
# ---------------------------------------------------------------------------
def demo_multi_turn(llm: AzureChatOpenAI) -> None:
    print("\n" + "=" * 60)
    print("Demo 3: Multi-Turn Conversation")
    print("=" * 60)

    history: list = [
        SystemMessage(content="You are a knowledgeable assistant. Keep answers brief."),
    ]

    turns = [
        "What is LangChain?",
        "How does it integrate with Azure OpenAI?",
        "Give me a one-line code example of that integration.",
    ]

    for user_input in turns:
        history.append(HumanMessage(content=user_input))
        print(f"\nUser: {user_input}")

        response = llm.invoke(history)
        history.append(AIMessage(content=response.content))
        print(f"Assistant: {response.content}")


# ---------------------------------------------------------------------------
# Entry point
# ---------------------------------------------------------------------------
if __name__ == "__main__":
    llm = get_llm()

    demo_simple_chat(llm)
    demo_prompt_chain(llm)
    demo_multi_turn(llm)

    print("\n" + "=" * 60)
    print("All demos completed successfully.")
    print("=" * 60)
