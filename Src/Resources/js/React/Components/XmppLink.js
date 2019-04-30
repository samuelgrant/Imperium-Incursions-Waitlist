import React, { Component } from 'react';

export default class XmppLink extends Component {

    createXmppUrl(authName) {
        return `xmpp:${authName.toLowerCase().replace(/ /g, "_")}`;
    }

    AuthName() {
        return this.props.AuthName || "";
    }

    render() {
        return ( <a href={this.createXmppUrl(this.AuthName())}>{this.AuthName()}</a> )        
    }
}