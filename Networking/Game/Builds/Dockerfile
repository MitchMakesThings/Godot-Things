FROM debian:stable-slim

EXPOSE 4242/udp

ADD whale-game.x86_64 /app/whale-game
ENTRYPOINT ["/app/whale-game", "--server", "--headless"]