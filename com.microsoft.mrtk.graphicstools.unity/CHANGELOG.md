# Changelog

All notable changes to this package will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/).

## [0.8.8] - 2025-05-29

### Changed

- Fixed a bug where scenes in packages could not be light combined.

## [0.8.7] - 2025-04-23

### Changed

- Fixed a bug where source and destination blend alpha were not maintained when "Allow Material override" was checked.
- Fixed a Unity 2021 shader compilation error.
- Acrylic bug fix, UNITY_UV_STARTS_AT_TOP is always defined, so the previous check was incorrect.
- @vmoras6699 changed the scalable shader graph target's unlit low shader path to use baked lit.

## [0.8.6] - 2025-04-21

### Changed

- Reduce the number of samplers AreaLights use.

## [0.8.5] - 2025-04-18

### Changed

- Fix corrupt .meta file.

## [0.8.4] - 2025-04-03

### Changed

- Added more properties to the experimental AreaLight component.

## [0.8.3] - 2025-03-26

### Changed

- This change introduces a new experimental light type called an AreaLight - area lights allow light to emit from a polygonal surface (quad) rather than a single point.

## [0.8.2] - 2025-03-13

### Changed

- Added the LightCombinerWindow which is an editor window that provides a user interface to combine light maps with albedo textures.

## [0.8.1] - 2024-12-02

### Changed

- Added support for Unity URP SSAO's "Depth normal" mode.

## [0.8.0] - 2024-11-21

### Changed

- Added scalable shader graphs support and improved backplate shaders.

## [0.7.3] - 2024-10-202

### Changed

- Fixed APIs deprecated in Unity 6 and ensured that the newly created MaterialInstances inherit all properties from the source material.

## [0.7.2] - 2024-07-26

### Changed

- Added a pre-initialization event to the AcrylicLayerManager.

## [0.7.1] - 2024-05-22

### Changed

- Fixed Unity 2023 Text Mesh Pro rendering issues.

## [0.7.0] - 2024-05-07

### Changed

- Fixed Unity 2023 compilation errors.

## [0.6.9] - 2024-02-27

### Changed

- Fix shader include issue with embedded packages.

## [0.6.8] - 2024-02-16

### Changed

- Updated CanvasFrontplate shader to apply edge outline mask to alpha channel

## [0.6.7] - 2024-01-18

### Changed

- Added rounder corner support to the magnifier shader and sample.

## [0.6.6] - 2023-12-01

### Added

- A CHANGELOG.md file to track changes across releases.

### Changed

- Converted `~Sample`` jpg images to png images.
