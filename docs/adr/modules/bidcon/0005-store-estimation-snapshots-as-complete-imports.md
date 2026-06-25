# Store estimation snapshots as complete imports

`EstimationSnapshot` stores a complete imported Bidcon estimation state, not only the data needed by a particular view. This increases storage and import cost, but preserves historical truth so future views, comparisons and report renders do not depend on re-fetching old data from Bidcon.
