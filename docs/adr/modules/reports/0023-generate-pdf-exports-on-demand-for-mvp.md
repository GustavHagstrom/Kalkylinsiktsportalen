# Generate PDF exports on demand for MVP

MVP PDF export should generate the PDF on demand from the active rendered view state rather than storing every export as a historical artifact. If saved export artifacts become important later, introduce a `ReportExportRecord` with view, snapshot/project version, change set version, exported-by, exported-at and file reference metadata.
