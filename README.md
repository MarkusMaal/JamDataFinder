# JAM data finder

This tool allows you to find sequence and voicebank data from PSX/PS2 games that use the SSsq/SShd format (a.k.a. JAM). Mostly found in games developed by Sony.

## Features

- Search for SShd headers inside binary containers (such as memory dumps, DAT/BIN/etc. containers)
- Search for a matching .BD file based on the offsets in SShd header
- Search for SSsq sequences
- Low memory usage

## Converting JAM to DLS and MIDI

This tool by itself can't do this, but you can try using these tools:

- [vgm2dls](https://github.com/AceKombat/vgm2dls) - allows you to convert HD/BD files to DLS soundfont file
- [vgmseq2midi](https://github.com/AceKombat/vgmseq2midi) - for getting the MIDI data from the SQ file
- [MidiEditor](http://midieditor.org/) - allows you to edit MIDI files, useful if you want to fix MIDI files in a way where it doesn't use percussion channel as vgmseq2midi/vgm2dls don't account for this
