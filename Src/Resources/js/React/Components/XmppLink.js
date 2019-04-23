import React, { Component } from 'react';

export default class XmppLink extends Component {

    createXmppUrl(authName) {
        return `xmpp:${authName.toLowerCase().replace(' ', '_')}`;
    }

    AuthName() {
        if (this.props.AuthName)
            return this.props.AuthName;

        return "";
    }

    render() {
        return ( <a href={this.createXmppUrl(this.AuthName())}>{this.AuthName()}</a> )        
    }
}