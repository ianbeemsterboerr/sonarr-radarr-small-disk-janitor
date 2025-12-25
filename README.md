# Janitor

Janitor is a small, lightweight tool that polls Plex frequently, after which it polls Sonarr and Radarr for movies and show episodes that it still has files for, even when they have been watched already. When it finds files in Sonarr and Radarr, it deletes them.
This tool is useful for Plex setups with Sonarr and or Radarr that are not being hosted for bigger groups of users, as it just checks if a show/movie has been watched at least once, after which it deletes it from the source tool.
Another requirement for usage would be a rather small disk that fills up quite quickly. This tool is NOT for enthousiasts of having a large library of media.

TODO is still a Radarr integration.
