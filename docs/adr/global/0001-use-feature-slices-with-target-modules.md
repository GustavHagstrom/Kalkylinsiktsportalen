# Use feature slices with target modules

Product code should live in the module that owns the behavior: `Projects`, `Reports`, `Bidcon` or `Access`. New use cases should be organized as feature slices inside the owning module, with explicit public contracts when another module or the host needs to compose the behavior.
