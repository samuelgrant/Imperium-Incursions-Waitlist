import React, { Component } from 'react';
import { render } from 'react-dom';
import Alert from './Components/Alert';
import UserSettings from './Components/UserSettings/UserSettings';
import Fittings from './Components/UserSettings/Fittings';

const baseUri = "/account-settings";

export default class AccountSettings extends Component {
    constructor(props) {
        super(props);

        this.state = {
            settings: null,
        }
    }

    componentDidMount() {
        this.getData();
    }

    //Ajax call to API to get data
    getData() {
        $.ajax({
            type: 'get',
            url: `${baseUri}/data`,
        }).done((data) => {
            this.setState({
                settings: data
            });
        }).fail((err) => {
            console.error(`[React/AccountSettings] @getData - Error retrieving account information`, err.responseText);
        })
    }

    render() {
        return (
            <div className="container">
                <div className="row">
                    <div className="col-lg-6 col-md-12">
                        <UserSettings settings={this.state.settings} forceUpdate={this.getData.bind(this)} baseUri={baseUri} />

                        <div className="clearfix"></div>

                        <Fittings fits={this.state.settings ? this.state.settings.fits : null} forceUpdate={this.getData.bind(this)} baseUri={baseUri} />
                    </div>

                    <div className="col-lg-6 col-md-12">
                        <Alert type="alert-primary"><bold>Pilot Skills goes here:</bold> This module is coming soon.</Alert>
                    </div>
                </div>
            </div>
        );
    }
}

if (document.getElementById('accountSettings'))
    render(<AccountSettings />, document.getElementById('accountSettings'));