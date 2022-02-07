# Compiling
First step is compiling the game.
Use the linux template, and specify a custom Release template of `./godot.linux`
Export a .pck, and replace `./moba.pck`

# Building docker image
Run the following to rebuild the docker image. This will use the new .pck you've just compiled!

`docker build -t mitchreidnz/godot-moba:runtime .`

# Running locally
The following command will run the server locally. You can connect to localhost on port 4242.

`docker run -p 4242:4242/udp mitchreidnz/godot-moba:runtime`

# Pushing updated docker image
Note that we should use a unique tag name, rather than just runtime.
Otherwise the server is unlikely to detect that the image has changed!

`docker push mitchreidnz/godot-moba:runtime`