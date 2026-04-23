# Specs

All Spec Kit feature specifications live here, one folder per feature branch.

## Structure

```
specs/
  <feature-branch-name>/
    spec.md          ← WHAT to build (user stories + acceptance criteria)
    plan.md          ← HOW to build it (technical design + file changes)
    data-model.md    ← data model / schema changes (if applicable)
    tasks.md         ← ordered task list with [P] parallelism markers
    research.md      ← research notes (from /speckit.analyze)
    contracts/       ← API contracts, port interfaces
```

## Workflow

```
/speckit.specify  →  /speckit.clarify  →  /speckit.plan  →
/speckit.analyze  →  /speckit.tasks    →  /speckit.implement
```

> Constitution: `.specify/memory/constitution.md`  
> Extensions: `specify extension list`
