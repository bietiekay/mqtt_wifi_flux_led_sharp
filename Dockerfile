FROM mono:onbuild

RUN mkdir /configuration
VOLUME /configuration

COPY wifi-led.json /configuration/wifi-led.json

CMD [ "mono",  "./wifi_led_strip.exe" ]
