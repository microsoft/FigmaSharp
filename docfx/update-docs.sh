#!/bin/sh

docfx && rm -Rf ../docs/ && cp -R ./_site ../docs
