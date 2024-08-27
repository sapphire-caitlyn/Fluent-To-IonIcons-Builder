This package ports the Fluent UI System Icons to use with IonIcons in an easy way.

## How to use
The name convetion used is the same as ionicons, being
```html
<ion-icon name="fluent-[icon-name]"></ion-icon>
```
for filled icons

and
```html
<ion-icon name="fluent-[icon-name]-outline"></ion-icon>
```
for regular icons

It should be noted that you should update the assets imports in `angular.json`
```json
"assets": [
  {
    "glob": "**/*.svg",
    "input": "node_modules/ionicons/dist/ionicons/svg",
    "output": "./svg"
  },
  {
    "glob": "**/*.svg",
    "input": "node_modules/Output",
    "output": "./svg"
  }
]
```

The source code of the exports can be found at https://github.com/sapphire-caitlyn/Fluent-To-IonIcons-Builder