# Script Explanations

Since this asset focuses on stealth AI, the remaining scripts exist mainly for demo and showcase purposes.  
Below are the explanations of the core systems.

------------------------------------

## BasicEnemyAI.cs
You must assign a **Player** (or any target you want the AI to detect).  
If left empty, the AI will continue patrolling without detecting anything.

### Settings

**Detection Range**  
Distance at which the AI can start detecting the player. Increasing this allows detection from farther away.

**Vision Range**  
Distance at which the AI loses sight of the player after initial detection.

**Walking & Running**  
Movement speeds of the AI. The AI switches to running during the **Chase** state.

**Attack Range**  
Enemy attack distance. Increasing this allows enemies to attack from farther away.

**Min Wait & Max Wait Seconds**  
How long the AI waits at a patrol point before moving again.

**Check Cooldown**  
Controls how fast the suspicion value decreases.  
Higher values = faster cooldown.  
(Recommended range: 0.1 – 1)

**Suspicion Time**  
Base time required for the AI to fully detect the player.  
Higher value = slower detection.

**Alarm Multiplier**  
Final detection time = `Suspicion Time × Alarm Multiplier`.

**Vision Mask**  
Layer mask determining what the AI can see or ignore.

**Search Duration**  
How long the AI searches after losing sight of the player.

**View Angle**  
Normal field of view angle.  
Visible via the spotlight attached to enemies.

**Chase View Angle**  
Field of view while chasing the player.

**Adjust View Angle**  
Fine tuning value for detection angle.  
If set above 1, the AI may detect the player slightly outside the visible spotlight cone.  
(Does not visually affect the spotlight.)

**Suspicion Timer**  
Debug-only value showing current suspicion level.

------------------------------------

## PatrolPointManager.cs
Assign enemies and patrol points in the scene.  
This script distributes patrol points randomly among enemies.

If you do not want to use this system:  
Open **BasicEnemyAI.cs**, remove `[HideInInspector]` from:
`public Transform[] patrolPoints;`

Then manually assign patrol points to each enemy.

**Min Assigned Patrol Points**  
Minimum number of patrol points per enemy.  
(Recommended: at least 2)

------------------------------------

## AlarmEnemy.cs
Triggers a global alarm across all enemies.

When one enemy detects the player, all enemies become alerted.

Usage:
1. Create an empty GameObject in the scene  
2. Attach this script to it  

------------------------------------

## Controller.cs
Simple demo character controller using Unity's **Character Controller** component.  
Included for testing and demonstration purposes.

------------------------------------

# Dependencies

**Unity Input System**  
Must be enabled from:  
`Edit → Project Settings → Player → Active Input Handling → Input System Package`

**TextMeshPro**  
Required for UI. Included by default in most Unity projects.

**NavMesh Surface & NavMesh Agent**  
This asset uses Unity's NavMesh Surface and NavMesh Agent systems for enemy movement and pathfinding.

------------------------------------

# Known Issues

**Inconsistent detection**  
If the player moves very fast and very close in front of an enemy, the AI may not immediately switch to the alarm state.  

This can be adjusted by tweaking:  
- **Suspicion Time**  
- **Alarm Multiplier**