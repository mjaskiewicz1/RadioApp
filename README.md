# RadioApp

Prosta aplikacja w .NET MAUI do odtwarzania internetowych stacji radiowych.

Projekt powstał **wyłącznie w celach edukacyjnych i portfolio**. Nie jest przeznaczony do użytku komercyjnego.

Projekt skupia się przede wszystkim na platformie **Android**.

## Stacje radiowe

Lista stacji oraz adresy streamów są pobierane z publicznego katalogu [Radio Browser](https://www.radio-browser.info/) przez [Radio Browser API](https://de1.api.radio-browser.info).

Do komunikacji z Radio Browser wykorzystuję własny klient API, który napisałem samodzielnie: [Clients](https://github.com/mjaskiewicz1/Clients).

Aplikacja nie hostuje ani nie retransmituje strumieni radiowych — korzysta z danych udostępnianych przez Radio Browser API.

## VLC

Do odtwarzania wykorzystywane są **LibVLCSharp** oraz **libVLC** firmy VideoLAN.

Licencje:

- [LibVLCSharp — LGPL 2.1](https://code.videolan.org/videolan/LibVLCSharp/-/blob/master/LICENSE)
- [VLC / libVLC — LGPL 2.1](https://code.videolan.org/videolan/vlc/-/blob/master/COPYING.LIB)

Projekt nie jest oficjalnie powiązany z VideoLAN ani Radio Browser.
