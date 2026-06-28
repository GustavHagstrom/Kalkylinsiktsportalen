# Limit Shared to stable cross-cutting building blocks

`Shared` should shrink over time and only contain genuinely shared building blocks such as stable kernel types, generic UI primitives and technical infrastructure. Product logic should belong to the owning feature or target module so `Shared` does not become the default place for unrelated behavior.
