# Repagination Is Driven by Document Change Token

The paginated document renderer repaginates when the printable document identity, required change token, or page format changes, or when an explicit refresh is requested. It does not repaginate on every Blazor `StateHasChanged`, because many component renders do not affect printable layout.

Report content changes that can affect measured height, such as a user editing a row or changing view-scoped report data, must produce a new printable document change token. The token is opaque to the renderer and is only compared for change; static documents can use a stable token such as `static`.
