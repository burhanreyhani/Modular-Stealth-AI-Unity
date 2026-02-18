# Changelog

## [1.0.0] - Initial Release

### Added
- Enemy AI system with Patrol / Check / Chase / Search / Attack states
- Dynamic patrol point distribution system (randomized per enemy)
- Suspicion-based detection & alert system with smooth light feedback
- Global alarm system (all enemies react when one detects the player)
- NavMesh-based movement and search behavior
- Player detection using distance, view angle, and raycast visibility

### Notes
- AI enemies can pass through each other to prevent getting stuck during navigation.
- Designed primarily for learning purposes and rapid prototyping.

### Known Issues
- **Inconsistent detection:**  
  If the player moves very fast and very close in front of an enemy, the enemy may not immediately switch to the alarm state.  
  This can be adjusted by tweaking **Suspicion Time** and **Alarm Multiplier** values.