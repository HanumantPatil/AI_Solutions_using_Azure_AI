"""Sample AgentExecutor implementation used by the A2A SDK server."""

from __future__ import annotations

import logging
from typing import Optional

from a2a.server.agent_execution import AgentExecutor, RequestContext
from a2a.server.events import EventQueue
from a2a.types import Task, TaskState, TaskStatus, TaskStatusUpdateEvent
from a2a.utils import new_agent_text_message

logger = logging.getLogger(__name__)


class SampleAgentExecutor(AgentExecutor):
    """Processes tasks and emits a simple text response."""

    async def execute(self, context: RequestContext, event_queue: EventQueue) -> None:
        task_id = context.task_id
        context_id = context.context_id
        logger.info("Executing task %s", task_id)

        # Get user input from request context
        user_message = context.get_user_input()
        if not user_message:
            await event_queue.enqueue_event(
                TaskStatusUpdateEvent(
                    task_id=task_id,
                    context_id=context_id,
                    status=TaskStatus(state=TaskState.failed),
                    final=True,
                )
            )
            return

        response_text = self._generate_response(user_message, task_id)
        response_message = new_agent_text_message(response_text)

        await event_queue.enqueue_event(
            TaskStatusUpdateEvent(
                task_id=task_id,
                context_id=context_id,
                status=TaskStatus(
                    state=TaskState.completed,
                    message=response_message,
                ),
                final=True,
            )
        )

    async def cancel(self, context: RequestContext, event_queue: EventQueue) -> None:
        task_id = context.task_id
        context_id = context.context_id
        logger.info("Cancelling task %s", task_id)
        await event_queue.enqueue_event(
            TaskStatusUpdateEvent(
                task_id=task_id,
                context_id=context_id,
                status=TaskStatus(state=TaskState.canceled),
                final=True,
            )
        )

    def _generate_response(self, user_message: str, task_id: Optional[str]) -> str:
        return (
            "I am a sample A2A agent using the official SDK. "
            f"You said: '{user_message}'. "
            f"Task ID: {task_id}."
        )
