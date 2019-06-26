import React, { Component } from 'react';

export default class UserSettings extends Component {
    notificationsEnabled() {
        return this.props.settings && this.props.settings.jabberNotifications;
    }

    handleCheckboxClick() {
        $.ajax({
            type: 'post',
            url: `${this.props.baseUri}/jabber`,
            data: {notificationsEnabled: !this.notificationsEnabled()}
        }).done((data) => {
            this.props.forceUpdate();
        }).fail((err) => {
            console.error(`[React/UserSettings] @handleCheckboxClick - Error setting the users jabber settings`, err.responseText);
        })
    }

    render() {
        return (
            <div>
                <h3>Settings</h3>
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
                        </ul>
                    </li>
                </ul>
            </div>
        )
    }
}