# Unity UI Theming

Add a UI manager to your scene and import one of the example themes to get started.

## How to use

This unity tool consists of four core components:

- Skins
- Theme Elements
- Multi Image Targets
- Multi Image UI Components

### Theme Elements

Every Theme Element is a collection of themes - colours (`Graphic Themes`), gradients (`Gradient Themes`), and text (`Text Themes`). Theme Elements may contain many themes within them. 
Each theme represents the theming preference for one component. Each Theme can have seperate settings per UI object state (Normal/Hover/Pressed/Disabled).

For example, a Button may have a `Graphic Theme` for its outline, shadow and icon, a `Gradient Theme` for the main button backdrop, and a `Text Theme` for the text on button.

### Skins

`Skins` are simply a list of `Theme Elements`. A `UI Manager` component in the scene holds a reference to the currently active skin.

You can swap the skin referenced in the `UI Manager`, and providing that the new skin contains Theme Elements with the same names, objects in the scene will update their theme to match the new skin.

### Multi Image Targets

`Multi Image Targets` are components that sit on the UI element that is to be themed. They hold a reference to each `Graphic`, `Gradient`, and `Text` that will have the `Theme Element` applied to.

All of Unity's UI elements only allow for a single item to be changed with a state, the implementation of `Multi Image Targets` means that you can change any number of child elements at the same time.

The order of the components in each `Graphic`, `Gradient`, and `Text` lists directly corresponds to the order in the `Theme Element`, e.g. the first `Graphic Theme` in the `Theme Element` will be applied to the first `Graphic Target` in the `Multi Image Target`.

Multi Image Targets typically live on the parent of the UI component being themed.

### Multi Image UI Components

These are components that you will need to use in place of Unity's standard UI Components. They extend the standard UI components & essentially call the `Multi Image Targets` on state change, so that all the related UI components change on state change, instead of just one.

Included are:

- `MultiImageButton`
- `MultiImageSlideToggle`
- `MultiImageToggle`
- `MultiImageToggleGroup`
- `SmoothDropdown`

## Included Samples

Included with the UI Theming Package are a few examples:

- **Ultraleap Colour Swatches** - a collection of swatches that can be accessed in the colour picker, based our brand's Design System
- **Ultraleap Dark Theme** - the theme used for the Ultraleap Control Panel
- **Ultraleap Dark Theme 24** - the theme used for the Ultraleap XR Launcher rebrand in 2024.
  - Includes a set of reusable UI prefabs based on the launcher

## Included Tools

There are also some useful tools included in the package:

- **Rounded Corners**: Apply smooth rounded corners to UI components, without needing to mess around with slicing
- **Gradients**: Apply a gradient to a UI image
- **Drop Shadow**: Add a drop shadow to a UI component (non compatible with compressible UI)
- **Slide Toggle Label**: A label that updates based on the toggle it is paired wit
- **Toggle Icons**: A script that swaps an image based on the toggle state it is paired with
- **UltraleapColours**: A static class that lets you reference brand colours whilst scripting
- **Sliced Filled Image**: A way to fill images up that are sliced (letting you easily make progress bars that have rounded corners)
