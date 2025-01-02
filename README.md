# Unity First Person Head Bob & Stamina System

A comprehensive first-person head bobbing and stamina system for Unity that includes dynamic movement effects, stamina management, and footstep audio.

## Features

### Head Bobbing
- Smooth figure-8 movement pattern for realistic head motion
- Different intensities for walking and sprinting
- Dynamic tilt and rotation effects
- Smooth transitions between movement states
- Progressive breathing effects tied to stamina levels

### Stamina System
- Dynamic stamina bar UI with fade effects
- Smooth stamina drain during sprinting
- Gradual stamina regeneration when resting
- Visual feedback through UI
- Affects sprint capabilities and movement

### Audio System
- Separate footstep sounds for walking and sprinting
- Random footstep selection without repetition
- Dynamic breathing sounds based on exhaustion
- Independent audio sources for footsteps and breathing

## Setup

1. Attach CompleteHeadBob script to your main camera
2. Attach StaminaBar script to your UI canvas
3. Set up the required UI elements for the stamina bar
4. Configure AudioSources for footsteps and breathing
5. Add your footstep sound clips to the respective arrays

## Components Required

- Character Controller
- Main Camera
- Canvas (for UI)
- Audio Sources (for footsteps and breathing)
- UI Images (for stamina bar)

## Script Descriptions

### CompleteHeadBob.cs
Handles head bobbing movement, stamina management, and audio effects.

### StaminaBar.cs
Manages the stamina UI, including fade effects and visual feedback.

## Configuration

Adjust the following parameters in the inspector to customize the system:

### Movement Settings
- Walk/Sprint Bob Speed
- Vertical/Horizontal Bob Amounts
- Tilt Amounts
- Forward Bob Amounts

### Stamina Settings
- Max Stamina
- Drain/Regen Rates
- Exhaustion Threshold

### UI Settings
- Bar Size
- Fade Speeds
- Colors

### Audio Settings
- Footstep Sounds
- Breathing Effects
- Volume Levels
