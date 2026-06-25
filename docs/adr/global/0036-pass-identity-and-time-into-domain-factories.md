# Pass identity and time into domain factories

Use cases should generate or obtain IDs and timestamps, then pass them into domain factories and behavior methods. Domain models should validate and use those values, but should not call clocks, random generators or persistence services directly, keeping aggregate behavior deterministic and easy to test.
