#!/bin/sh

docfx --intermediateFolder ./obj/.cache/build/ && cp -R ./_site ../docs && rm -Rf ./_site

