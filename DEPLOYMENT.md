# Deployment

## Release
To push a new release, simply [draft a new one](https://github.com/assureddt/pact/releases/new) and give it a Tag version of the format "v1.0.0".
An action will then trigger a release of all packages with that version (with the v prefix removed for the assemblies).

## Wiki
The documentation Wiki content is auto-generated by flipping the "DocGen" [Directory.Buid.props](./src/Directory.Build.props) parameter to "true" then re-building the solution. The resulting markdown structure will be produced in a "Pact.Wiki" folder adjacent to where your "Pact" solution folder is located. That folder should be a clone of the Pact.Wiki repository, which acts like any other gihub repository, just commit and push to update the live wiki. That said, feel free to just ping @welshronaldo to request this if you can't make sense of it.

*N.B. this is very easily automated in a github action, however, the credentials (user-level access token) required to push to the separate wiki repo feels like an unpleasant hack so avoiding for now in the hope github provide a better option)*