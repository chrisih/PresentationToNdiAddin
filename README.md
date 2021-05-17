# Presentation to NDI
I created this PowerPoint VSTO AddIn for our church.
We are using NDI there for live streaming, and for the sermon we wanted to use PowerPoint - as it is easy for everyone to create something beautiful there, and many people already have it on their PC.

In the beginning we used a PowerPoint AddIn (self-created) that exports each slide as an image with transparent background, and we mixed this image as layer on top of the live stream (using OBS).
- first disadvantage of this approach was, that the export just works for "static" content like images and other shapes. It does not work for videos or animations
- second disadvantate is, that the exported image will not be shown if something goes wrong during creation of the image or during reading

Instead of breaking my head by solving these problems, we decided to change the current AddIn:
- the picture should not be exported as PNG, we want to use NDI here as well
- the AddIn should create a second NDI stream containing the grabbed presentation

While researching and testing it turned out, that the easiest way of implementing a screen grabber was bad in performance. So we changed from using GDI to usage of UWP components - which is a bit tricky but works well. Additionally, this method of grabbing uses the power of the graphics chip, so the CPU is not utilized that much.

We still did not reach the goal - currently, we have some known issues:
- when a presentation is started in fullscreen this breaks the AddIn
- no audio is captured at the moment. 
 
Additionally, we are creating an own ribbon to set several options (like for example the NDI Framerate etc).

The final goal is to have the AddIn running (of course) and
- have two NDI sources, one with the static content (and transparent background) and
- one with the grabbed presentation (including Audio)
