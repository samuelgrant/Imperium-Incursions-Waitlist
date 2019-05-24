import React, { Component } from 'react';

export class XmppLink extends Component {

    createXmppUrl(authName) {
        return `xmpp:${authName.toLowerCase().replace(/ /g, "_")}`;
    }

    AuthName() {
        return this.props.AuthName || "";
    }

    render() {
        return ( <a class="xmpp" href={this.createXmppUrl(this.AuthName())}>{this.AuthName()}</a> )        
    }
}

export class MumbleLink extends Component {

    mumbleLink() {
        if (this.props.commChannel)
            return this.props.commChannel.linkText || "Click to Join Comms";

        return;
    }

    createMumbleUrl() {
        if (this.props.commChannel)
            return this.props.commChannel.url;

        return;
    }

    render() {
        return (
            <span>
                <i className="fas fa-headset"></i>
                <a className="xmpp" href={this.createMumbleUrl()}>{this.mumbleLink()}</a>
            </span>
        )
    }
}