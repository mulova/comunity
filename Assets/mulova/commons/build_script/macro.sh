#!/bin/bash

ASSETS_DIR=$1

function remove_examples
{
    for line in $(find $ASSETS_DIR -type d -name 'Examples'); do
        rm -rf $line
    done
}
