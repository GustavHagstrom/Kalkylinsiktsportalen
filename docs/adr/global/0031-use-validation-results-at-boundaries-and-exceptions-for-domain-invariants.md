# Use validation results at boundaries and exceptions for domain invariants

Use cases should return validation results for expected request, input, permission and existence problems that the UI or API can present to a user. Domain models should protect their invariants by throwing domain exceptions when code attempts an operation that would put an aggregate into an invalid state, keeping aggregate behavior explicit instead of turning every domain method into a nullable or optional validation flow.
