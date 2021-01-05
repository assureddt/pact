# Pact.TagHelpers 🏷
Contains several general purpose ASP.NET Core TagHelpers (which replaced the old @helper Razor MVC feature).

Some examples follow:
## [AlertTagHelper](./AlertTagHelper.cs)
Renders a Bootstrap 4 compatible alert (dead simple and fairly pointless but reduces ceremony somewhat)
```html
<div alert-type="success" alert-message="You got this"></div>
<div alert-type="warning">Caution advised</div>
```

## [ConditionTagHelper](./ConditionTagHelper.cs)
Simply conditional rendering of an element based on a boolean condition without needing to wrap it in a Razor "@if {}" block
```html
<div condition="@myvariable">Something</div>
```

## [IsReadOnlyTagHelper](./IsReadOnlyTagHelper.cs)
Should hopefully apply the correct set of arbitrary attributes to an input element that makes it behave in a read-only manner
```html
<input type="text" is-read-only="@myvariable"/>
<input type="checkbox" is-read-only="@myvariable"/>
<input type="radio" is-read-only="@myvariable"/>
<select type="text" is-read-only="@myvariable">...</select>
```

The API Wiki can be found [here](https://github.com/assureddt/pact/wiki/Pact-TagHelpers-Index)
