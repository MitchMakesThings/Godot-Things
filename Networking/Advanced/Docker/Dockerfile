FROM ubuntu:latest

EXPOSE 4242/udp

RUN apt-get update && apt-get install -qq libxcursor-dev libxinerama-dev libxrandr-dev libxi-dev libglu-dev

ADD godot.linux /godot/
ADD moba.pck /godot/

WORKDIR /godot

ENTRYPOINT ./godot.linux --main-pack moba.pck --server --headless