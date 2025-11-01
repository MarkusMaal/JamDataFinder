# JAM data finder

This tool allows you to find sequence and voicebank data from PSX/PS2 games that use the SSsq/SShd format (a.k.a. JAM). Mostly found in games developed by Sony.

## Features

- Search for SShd headers inside binary containers (such as memory dumps, DAT/BIN/etc. containers)
- Search for a matching .BD file based on the offsets in SShd header
- Search for SSsq sequences
- Low memory usage

## Converting JAM to SF2 and MIDI

This tool by itself can't do this, but you can try using these tools:

- [FlipnicFileTools](https://github.com/MarkusMaal/FlipnicFileTools) - created by me, specialized for Flipnic, but also works with other games when it comes to converting .HD/.BD to .SF2 if you convert the sequence to MIDI yourself first
- [Sssq2mid](https://www16.atwiki.jp/soundfile?cmd=upload&act=open&pageid=6&file=Sssq2mid.7z) - for getting the MIDI data from the SQ file