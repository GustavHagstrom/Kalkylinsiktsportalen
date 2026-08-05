# Paginated Document Renderer Boundary

Reports use a `PaginatedDocumentRenderer` as a module-owned layout engine for fixed-format printable documents. It owns page preview state, pagination errors, and the safe print command, but it does not know about `View`, `RenderingContext`, Bidcon, or report-domain rules.

The renderer flattens body content into indivisible placement blocks and places those blocks into the body region between repeated document header and footer regions. Content that cannot be placed correctly, such as a placement block taller than the available body area, is a blocking pagination error and prevents print/PDF output. Initial PDF output uses the same paginated DOM as browser print-to-PDF so preview, print, and PDF share one WYSIWYG contract.
