# Start Projects slices with domain and in-memory stores

Initial Projects slices should prove aggregate behavior and use case contracts with domain tests and in-memory repositories before adding MongoDB persistence. Mongo implementations should follow soon after the domain shape is stable, but persistence mapping should not drive the domain model.
