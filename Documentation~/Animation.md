---
title: Profiling
description: Guide to graphic resources and techniques in Graphics Tools.
author: Cameron-Micka
ms.author: thmicka
ms.date: 12/12/2020
ms.localizationpriority: high
keywords: Unreal, Unreal Engine, UE4, HoloLens, HoloLens 2, Mixed Reality, development, MRTK, GT, Graphics Tools, graphics, rendering, materials
---

# Profiling

The easiest way to rationalize performance is via framerate or how many times your application can render an image per second. It is important to meet the target framerate, as outlined by the platform being targeted (i.e [Windows Mixed Reality](https://docs.microsoft.com/windows/mixed-reality/understanding-performance-for-mixed-reality), [Oculus](https://developer.oculus.com/documentation/pcsdk/latest/concepts/dg-performance-guidelines/), etc). For example, on HoloLens, the target framerate is 60 FPS (or 16.66 milliseconds). Low framerate applications can result in deteriorated user experiences such as worsened [hologram stabilization](../hologram-Stabilization.md), world tracking, hand tracking, and more. To help developers track and achieve quality framerate, Graphics Tools provides profiling tools and best practices.

![Profiling](Images/FeatureCards/Profiling.png)

## Visual profiler

To continuously track performance over the lifetime of development, it is highly recommended to always show a framerate visual while running & debugging an application. Graphics Tool's provides the `GTVisualProfiler` actor which gives real-time information about the current frame times represented in milliseconds, draw call count, and visible polygon count in a stereo friendly application view.

> [!NOTE]
> To utilize the visual profiler simply drop an instance of the `GTVisualProfiler` actor in a level. To disable the profiler you can mark it as hidden in game (or not visible).

The visual profiler provides four metrics around frame performance and two metrics around level complexity as outlined below.

| Name       | Description                                                                                                                                                                                                                                 |
|------------|---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| Frame      | Frame time represents the total amount of time spent generating one frame of the app. Since both the Game and Draw threads sync up before finishing a frame, frame time is often close to the time being shown in one of these threads. |
| Game       | If Frame time is close to Game time, the app's performance is likely being bottlenecked (negatively impacted) by the game thread. The game thread is resposible for most app logic, especially Blueprint logic.                             |
| Draw       | If Frame time is close to Draw time, the game's performance is likely being bottlenecked by the rendering thread. The rendering thread is responsible for deciding what to render and submitting work to the GPU.                         |
| GPU        | GPU time measures how long the video card took to render the scene. Since GPU time is synced to the frame, it will likely be similar to Frame time. Note, this value is not available on all rendering hardware interfaces.                                                                                        |
| Draw Calls | Draw calls can be thought of as the number on times a graphics API (such as [DirectX](https://en.wikipedia.org/wiki/DirectX)) is told to render an object.                                                                                  |
| Polys      | Represents the number of polygons which are currently being submitted to the graphics API for rendering. This number may vary slightly to actual number being rendered due to frustum clipping or geometry generation on the GPU.                    |

> [!NOTE]
> It is particularly important to utilize the visual profiler to track frame time when running on the device as opposed to running in editor or an emulator. The most accurate performance results will be depicted when running on the device with "Shipping" build configuration.

## Example level

To better understand the `GTVisualProfiler` look at the `\GraphicsToolsProject\Plugins\GraphicsToolsExamples\Content\Profiling\Profiling.umap` level.

## General recommendations

Performance can be an ambiguous and constantly changing challenge for mixed reality developers and the spectrum of knowledge to rationalize performance is vast. There are some general recommendations for understanding how to approach performance for an application though.

It is useful to simplify the execution of an application into the pieces that run on the *CPU* or the *GPU* and thus identify whether an app is bound by either component. There can be bottlenecks that span both processing units and some unique scenarios that have to be carefully investigated. However, for getting started, it is good to grasp where an application is executing for the most amount of time.

### GPU bound

If the longest bar on the profiler is the "GPU" time then your app is likely GPU bound.

> [!NOTE]
> The visual profiler screenshot at the top of this document would be considered GPU-bound.

Since most platforms for mixed reality applications are utilizing [stereoscopic rendering](https://en.wikipedia.org/wiki/Stereoscopy), it is very common to be GPU-bound due to the nature of rendering a "double-wide" screen. Futhermore, mobile mixed reality platforms such as HoloLens or Oculus Quest will be limited by mobile-class CPU & GPU processing power.

When an app is GPU bound try moving towards holograms until they fill your view. If the GPU time increases as holograms fill your view you are likely [fill rate](https://en.wikipedia.org/wiki/Fillrate) bound and should focus on reducing material complexity and ensuring no full screen effects (such as post processing or MSAA) are enabled. If your GPU time decreases as holograms fill your view and increases as your whole level comes into view you are likely vertex processing bound and need to simplify your meshes.

### CPU bound

If the longest bar on the profiler is the "Game" or "Draw" time, then your app is likely CPU bound.

Mixed reality applications are normally bound by the time it takes to render a level, the "Draw" time when CPU bound. The easiest way to reduce rendering time is to remove "Draw Calls" via instancing (Unreal Engine supports [auto-instancing on mobile](https://docs.unrealengine.com/en-US/SharingAndReleasing/Mobile/Rendering/HowTo/AutoInstancingOnMobile/index.html)) or merging draw calls together in your asset creation tools or within the editor using the [Merge Actor Tool](https://docs.unrealengine.com/en-US/Basics/Actors/Merging/index.html#:~:text=The%20Merge%20Actors%20tool%20enables,by%20the%20newly%20merged%20asset). 

If the "Game" thread is the longest bar on the profiler then you should look at optimizing logic in your Blueprint or C++ app code. For additional guidance please click on the links from the [see also](#See-also) section below. 

## See also

- [Lighting](Lighting.md)

### Windows Mixed Reality
- [Performance recommendations for Unreal](https://docs.microsoft.com/en-us/windows/mixed-reality/develop/unreal/performance-recommendations-for-unreal)
- [Material recommendations in Unreal](https://docs.microsoft.com/en-us/windows/mixed-reality/develop/unreal/unreal-materials)
- [Profiling with Unreal Insights](https://docs.microsoft.com/en-us/windows/mixed-reality/develop/unreal/unreal-insights)
- [Understanding Performance for Mixed Reality](https://docs.microsoft.com/windows/mixed-reality/understanding-performance-for-mixed-reality)

### Unreal
- [Performance guidelines for mobile devices](https://docs.unrealengine.com/en-US/SharingAndReleasing/Mobile/Performance/index.html)
- [VR performance testing](https://docs.unrealengine.com/en-US/SharingAndReleasing/XRDevelopment/VR/DevelopVR/Profiling/Overview/index.html)
- [Virtual reality best practices](https://docs.unrealengine.com/en-US/SharingAndReleasing/XRDevelopment/VR/DevelopVR/ContentSetup/index.html)