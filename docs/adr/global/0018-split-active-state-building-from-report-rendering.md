# Split active state building from report rendering

Projects builds the renderable snapshot state for a given view and snapshot/version by applying `ViewScopedChangeSet` and replacement policy. Reports then renders that active state with the view definition, sections, formulas, layout and PDF export, so calculation substitutions stay in Projects while presentation remains in Reports.
