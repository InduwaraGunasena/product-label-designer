# Product Label Designer

Product Label Designer is a Windows-based desktop application built using WPF (Windows Presentation Foundation) and .NET 8.0. It provides an interactive visual environment for designing and formatting product labels, managing metadata (such as dimensions and DPI margins), and positioning both fixed and variable data items.

The application supports real-time ruler alignments, zooming/panning, canvas recentering, data persistence to disk via JSON files, and future integrations for cloud storage providers.

![Working area](/images/1.png)

---

## Technical Stack & Dependencies

* **Framework:** .NET 8.0 (`net8.0-windows`)
* **Presentation Layer:** WPF (Windows Presentation Foundation)
* **Application UI:** `WinExe` output type with an implicit-usings and nullable-enabled workspace configuration.
* **Key NuGet Packages:**
* `Fluent.Ribbon` (v10.0.4) — Powers the Modern Office-styled Ribbon UI window layout.
* `Extended.Wpf.Toolkit` (v4.7.25104.5739) — Supplies custom UI control components like Zoomboxes and advanced window tooling.
* `EPPlus` (v7.6.0) — Integrated for spreadsheet manipulation and data source imports.
* `Newtonsoft.Json` (v13.0.3) — Utilized alongside native serializers for JSON management.



---

## Project Structure & Architecture Overview

The system is organized into decoupled modules separating visual layout components, core internal domain definitions, persistence operations, and UI helpers:

```text
product-label-designer/
│
├── win_app.sln                  # Visual Studio solution workspace configuration
└── win_app/
    ├── win_app.csproj           # MSBuild project file outlining build rules and packages
    ├── App.xaml / App.xaml.cs   # Global application entry point and resource initialization
    ├── AssemblyInfo.cs          # Metadata attributes concerning assembly deployment
    │
    ├── Label/                   # Core Data Contract Layer
    │   ├── LabelDesign.cs       # Root document structure matching JSON definitions
    │   ├── LabelMetadata.cs     # Holds dimensional configuration properties (Width, Height, margins)
    │   ├── LabelItems.cs        # Collection grouping structures segregating items
    │   ├── LabelItem.cs         # Single discrete asset instance payload properties
    │   ├── LabelFormat.cs       # Aesthetic blueprint criteria bindings
    │   ├── LabelItemFormat.cs   # Element-level formatting metadata
    │   └── LabelFormats.cs      # Global styles map references
    │
    ├── Services/                # Infrastructure & I/O
    │   └── LabelDesignManager.cs# Serializes/Deserializes active documents to JSON formatted files
    │
    ├── Windows/                 # UI Window Elements
    │   ├── starting_window      # Landing user experience for initializing workspace sessions
    │   ├── file_opening_window  # Selection wizard for launching historical drafts
    │   ├── document_properties  # Configuration manager modal setup for label boundary values
    │   └── main_editing_window  # Central workspace utilizing Ribbon layouts, Zoombox canvases, and Rulers
    │
    ├── Elements/                # Reusable UI Controls
    │   ├── HorizontalRuler      # X-Axis custom DPI ruler reflecting real-time positioning updates
    │   ├── VerticalRuler        # Y-Axis custom DPI ruler reflecting real-time positioning updates
    │   ├── LeftPaneToolItem     # Individual operational control objects for tool management
    │   ├── LeftPaneViewModel    # Datacontext logic processing active tool selection state
    │   └── RightPaneLabelItems  # Interactive pane monitoring dynamic and static layer stacks
    │
    ├── Models/                  # Application State Objects
    │   ├── Shortcut.cs          # Structure mapping user action events to concrete keys
    │   └── ShortcutManager.cs   # Global hotkey mapping library orchestrating shortcut triggers
    │
    ├── Formatters/              # Business Logic & Component Registries
    │   ├── LabelItemTypeRegistry# Tracking registry managing standard layout variations
    │   ├── RelayCommand.cs      # Boilerplate ICommand implementation binding visual assets to code
    │   └── LabelPropertyViewModel # ViewModels representing component settings
    │
    ├── Converters/              # XAML Binding Value Converters
    │   ├── BoolToVisibilityConverter.cs
    │   └── IntToVisibilityConverter.cs
    │
    └── Assets/                  # Resource Subdirectories
        ├── icons/               # Toolbar imagery resource assets
        └── images/              # Layout placeholders and background screens

```

---

## Core Feature Implementation Details

### 1. Document Schema & Serialization

The standard target project save-state maps directly to an aggregated root node defined in `LabelDesign.cs`. It unifies label dimensions, content items, and layout specifications into structured blocks using explicit property tagging:

* **`label`**: Stores structural physical attributes (Width, Height, Margins) via `LabelMetadata`.
* **`items`**: Segregates assets into lists representing statically set data (`Fixed`) or placeholders resolved at runtime (`Variable`).
* **`formats`**: Manages explicit typography and aesthetic alignments mapped across design identifiers.

Persistence operations are handled asynchronously by `LabelDesignManager`, which processes this data using strict CamelCase formatting conventions.

### 2. Multi-Tiered Editing Canvas

The UI layout relies on nested layouts inside `main_editing_window.xaml` to create a realistic mock-up workspace:

* **`LabelHostCanvas`**: The primary backdrop canvas hosting interactive control structures.
* **Outer Label Canvas**: Renders a yellow bounding region representing the raw physical edge limitations of the stock media paper.
* **Inner Content Canvas (`LabelContentArea`)**: A nested workspace displaying a clean white field bounded by structural safety margins, where custom item layout placement happens.

### 3. Spatial Math & Adaptive Rulers

To keep track of object coordinates across different screen sizes, layout calculations use a reference standard of **96 DPI per 25.4 mm**. When a user pans or changes zoom levels, coordinate transformations translate the canvas properties down into viewport-relative positions:

```csharp
// Transforms viewport bounds dynamically to determine the rendering origin points
GeneralTransform transform = ZoomboxControl.TransformToVisual(LabelHostCanvas);
Point viewportTopLeftInCanvas = transform.Transform(new Point(0, 0));

```

These calculated offsets are sent directly to the `HorizontalRuler` and `VerticalRuler` controls to update the measurement markers in real time.

### 4. Interactive Navigation & Accessibility

* **Viewport Management:** Includes built-in support for centering canvas contents (`CenterContent()`), resizing bounds to fit the viewport (`FitToBounds()`), and manual slider adjustments from 10% up to maximum zoom configurations.
* **Keyboard & Mouse Event Routing:** Integrates custom shortcut key listeners via `ShortcutManager` alongside mouse-wheel hooks. Users can quickly trigger functions like `Ctrl + MouseWheel` or specific hotkeys to step canvas dimensions up or down fluidly.

---

## Build and Setup Procedures

### Prerequisites

* Ensure you have installed the **.NET 8.0 SDK**.
* A modern IDE companion like **Visual Studio 2022** or **VS Code** with full C# support extensions.

### Installation Steps

1. Open your terminal or command line prompt and shift focus down into the project sub-folder housing your design configuration project files:
```bash
cd .\product-label-designer-master\win_app

```


2. Run the .NET restore task to pull down down stream package dependencies (such as Extended WPF Toolkit, EPPlus, and Fluent Ribbon):
```bash
dotnet restore

```


3. Build and run the executable application locally:
```bash
dotnet run

```
