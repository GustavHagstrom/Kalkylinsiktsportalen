# Pass view change settings to Projects

Projects needs view scope to build active renderable state, but should not depend on Reports' internal `ViewDefinition`. Reports or the Web layer should pass a small contract such as `ViewId` and `EnableChanges` into Projects, keeping replacement validation independent from report layout, sections and formulas.
