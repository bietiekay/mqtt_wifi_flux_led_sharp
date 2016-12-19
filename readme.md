# Wifi LED Strip Implementation (fluxled / flux_led / flux-led) based upon existing Python and Ruby implementations

## implementation

As it stands right now this there is a wifi_led_strip project written in C# that will use a .json configuration file as it's configuration.

On start-up it will load the configuration and connect to the specified MQTT broker. Next it's going to subscribe to the MQTT topic defined there.

'{
  "wifi_led_mqtt": {
    "mqtt_broker_ip": "192.168.178.92",
    "mqtt_command_topic": "house/stappenbach/rgblight/#",
    "mqtt_filter_topic": "house/stappenbach/rgblight/"
  }
}'

It will wait for messages coming in to the bespoke mqtt_command_topic. In order to adress specific wifi leds by ip adress the topic subscription needs to include the # at the end.

The mqtt_filter_topic is being used to identify the IP part of the topic - everything in this filter topic will be used to trim at the start of the incoming topics.

So "house/stappenbach/rgblight/192.168.178.79" will refer to a wifi led on ip adress 192.168.178.79.

The messages coming in will have to look like this:

house/stappenbach/rgblight/192.168.178.79 #26ff00
house/stappenbach/rgblight/192.168.178.79 #0011ff
house/stappenbach/rgblight/192.168.178.79 #000000
house/stappenbach/rgblight/192.168.178.79 #0011ff

This tool manages for each known Wifi LED the current state (as long as it is running, non persistent) and is turning the led on and off as needed. So sending #26ff00 will basically turn on the LED and then set to a greenish color. A #000000 will trigger a turn-off signall to the Wifi Flux LED.

Thats all folks ;)

## Links
  - port flux-led python to c#
    - http://blog.repsaj.nl/index.php/2016/09/iot-aquarium-monitor-controlling-led-from-c/
  - http://sli.mg/rbDRF8
  - https://github.com/Danielhiversen/flux_led
  - https://github.com/beville/flux_led/blob/master/README.md
