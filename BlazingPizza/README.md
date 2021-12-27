# Build web applications with Blazor

[MS Learn: Build web applications with Blazor](https://docs.microsoft.com/en-us/learn/paths/build-web-apps-with-blazor/)

dotnet new blazorserver -f net6.0 -o BlazorLab.Server

dotnet watch run

dotnet new razorcomponent -n Todo -o Pages

dotnet new razorcomponent -n PizzaBrowser -o Pages

1. Bootstrapping the Application - how does the application loads blazor
    - Layout
    - FallBack
    - Navigation

2. Component
    - Parameters
    - Query Parameters
    - Two-way Binding and events
    - Validation

The name of a Blazor component must begin with an uppercase character.

```html
    @onclick:stopPropagation

    @onclick="() => currentCount++"

    @onclick='mouseEvent => HandleClick(mouseEvent, "Hello")'

    @onkeypress="ProcessKeyPress" @onkeypress:preventDefault 
```