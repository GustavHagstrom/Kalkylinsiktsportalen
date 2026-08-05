# Report Rendering Is Two-Stage

Report rendering is split into a report adapter and a pagination renderer. `ReportDocumentRenderer` translates a `View` and `RenderingContext` into a model-driven `PrintableDocument`, using Blazor `DynamicComponent` for component types and parameters when needed. `PaginatedDocumentRenderer` receives the printable document and performs layout measurement, pagination, preview, and print without knowing where the report content came from.

This replaces the normal report path's need for a custom component HTML renderer. Dynamic component selection belongs to the report adapter; paginated layout belongs to the renderer.
