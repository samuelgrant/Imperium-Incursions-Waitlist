﻿import React, { Component } from 'react';

export default class ApplicationSettings extends Component {
    notificationsEnabled() {
        return this.props.settings && this.props.settings.allowsJabberNotifications;
    }

    handleCheckboxClick() {
        $.ajax({
            type: 'post',
            url: `${this.props.baseUri}/notifications`,
            data: {notificationsEnabled: !this.notificationsEnabled()}
        }).done((data) => {
            this.props.forceUpdate();
        }).fail((err) => {
            console.error(`React/Components/ApplicationSettings {ApplicationSettings@handleCheckboxClick} - Error setting the users jabber settings`, err.responseText);
        })
    }

    render() {
        return (
            <div>
                <h3>Application Settings</h3>
                <p className="ml-4">These settings allow you to configure specific settings for the waitlist:</p>
                <ul className="list-group pt-2">
                    <li className="list-group-item">
                        <label className="custom-control custom-checkbox">
                            <input type="checkbox" className="custom-control-input" onClick={this.handleCheckboxClick.bind(this)} defaultChecked={this.notificationsEnabled()} />
                            <span className="custom-control-indicator"></span>
                            <span className="custom-control-description">Allow the waitlist to send me Jabber PMs when:</span>
                        </label>

                        <ul className="list-unstyled pt-1">
                            <li><i className="fas fa-caret-right"></i> I am invited to a fleet</li>
                            <li><i className="fas fa-caret-right"></i> The FC is trying to get my attention</li>
                        </ul>
                    </li>
                </ul>
            </div>
        )
    }
}