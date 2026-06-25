# Use seeded snapshots before real Bidcon import

The first Projects slice should use seeded or test snapshots rather than depending on live Bidcon import. This lets Projects prove project, version and renderable state behavior independently, while a later Bidcon slice can introduce real API import and mapping; a manually exported Bidcon snapshot can be used later as a realistic fixture.
