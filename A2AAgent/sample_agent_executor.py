"""Sample AgentExecutor implementation used by the A2A SDK server."""

from __future__ import annotations

import logging
from typing import Optional

from a2a.server.agent_execution import AgentExecutor
from a2a.server.events import EventQueue
from a2a.types import Task, TaskState, TaskStatus, TaskStatusUpdateEvent
from a2a.utils import new_agent_text_message

logger = logging.getLogger(__name__)


class SampleAgentExecutor(AgentExecutor):
    """Processes tasks and emits a simple text response."""

    async def execute(self, task: Task, event_queue: EventQueue) -> None:
        logger.info("Executing task %s", task.id)

        user_message = self._get_last_user_message(task)
        if not user_message:
            await event_queue.enqueue_event(
                TaskStatusUpdateEvent(
                    taskId=task.id,
                    status=TaskStatus(state=TaskState.FAILED),
                    final=True,
                )
            )
            return

        response_text = self._generate_response(user_message, task)
        response_message = new_agent_text_message(response_text)

        await event_queue.enqueue_event(
            TaskStatusUpdateEvent(
                taskId=task.id,
                status=TaskStatus(
                    state=TaskState.COMPLETED,
                    message=response_message,
                ),
                final=True,
            )
        )

    async def cancel(self, task: Task, event_queue: EventQueue) -> None:
        logger.info("Cancelling task %s", task.id)
        await event_queue.enqueue_event(
            TaskStatusUpdateEvent(
                taskId=task.id,
                status=TaskStatus(state=TaskState.CANCELED),
                final=True,
            )
        )

    def _get_last_user_message(self, task: Task) -> Optional[str]:
        if not task.history:
            return None

        for message in reversed(task.history):
            if message.role == "user" and message.parts:
                texts = []
                for part in message.parts:
                    if hasattr(part, "text"):
                        texts.append(part.text)
                if texts:
                    return " ".join(texts)
        return None

    def _generate_response(self, user_message: str, task: Task) -> str:
        turn_count = sum(1 for m in (task.history or []) if m.role == "user")
        return (
            "I am a sample A2A agent using the official SDK. "
            f"You said: '{user_message}'. "
            f"Task ID: {task.id}. "
            f"This is turn {turn_count} in this conversation."
        )
