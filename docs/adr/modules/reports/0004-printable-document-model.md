# Printable Document Model

`PrintableDocument` is an immutable input model with an id, required opaque change token, page format, and header, body, and footer printable items. The selected page format belongs to the document because it is part of the report layout definition; for now `PageFormat` contains only named paper geometry such as A3, A4, or A5 with millimeter dimensions, while margins can become a separate page-layout concept later.

Printable items separate measurable `PrintableBlock` items from logical `PrintableContainer` items. A printable block has a stable id, component type, parameters, and page-break behavior, and is rendered with `DynamicComponent`. A printable container groups child printable items and can carry page-break behavior after the group without being measured or placed as an indivisible block.

Before measurement and pagination, containers are flattened into a linear list of placement blocks. `PageBreakBehavior` supports `None` and `After`; `After` on a container applies after its last placement descendant. Header, body, and footer share the printable-item model, but only body items participate in page placement.
