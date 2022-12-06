shader_type canvas_item;

uniform sampler2D destruction_mask : hint_default_black;

void fragment() {
	vec4 original_colour = texture(TEXTURE, UV).rgba;
	vec4 destruction_map_colour = texture(destruction_mask, UV).rgba;

	COLOR = vec4(original_colour.r, original_colour.g, original_colour.b, original_colour.a * destruction_map_colour.a);
}